package com.greedytown.domain.item.service;

import com.greedytown.domain.item.dto.AchievementsDto;
import com.greedytown.domain.item.dto.BuyItemDto;
import com.greedytown.domain.item.dto.BuyItemReturnDto;
import com.greedytown.domain.item.dto.ItemDto;
import com.greedytown.domain.item.model.Achievements;
import com.greedytown.domain.item.model.Item;
import com.greedytown.domain.item.model.ItemUserList;
import com.greedytown.domain.item.model.SuccessUserAchievements;
import com.greedytown.domain.item.repository.AchievementsRepository;
import com.greedytown.domain.item.repository.ItemRepository;
import com.greedytown.domain.item.repository.ItemUserListRepository;
import com.greedytown.domain.item.repository.SuccessUserAchievementsRepository;
import com.greedytown.domain.user.model.User;
import com.greedytown.domain.user.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;

@Service
@RequiredArgsConstructor
public class  ItemServiceImpl implements ItemService{

    private final ItemRepository itemRepository;
    private final UserRepository userRepository;
    private final ItemUserListRepository itemUserListRepository;
    private final SuccessUserAchievementsRepository successUserAchievementsRepository;
    private final AchievementsRepository achievementsRepository;

    //전체 아이템 보기
    @Override
    public List<ItemDto> getStoreItems() {
        List<ItemDto> itemDtoList = new ArrayList<>();
        for(Item item : itemRepository.findAll()){
            ItemDto itemDto = ItemDto.builder().
                    itemIndex(item.getItemIndex()).
                    itemCode(item.getItemCode()).
                    itemPrice(item.getItemPrice()).
                    itemName(item.getItemName()).build();
            itemDtoList.add(itemDto);
        }
        return itemDtoList;
    }
    //아이템 구입하기
    public BuyItemReturnDto buyStoreItem(BuyItemDto buyItemDto){

        //내 돈 없애기
        Long price = buyItemDto.getItemPrice();
        User user = userRepository.findById(buyItemDto.getUserIndex()).get();
        user.setUserMoney(user.getUserMoney()-price);
        Item item = itemRepository.findById(buyItemDto.getItemIndex()).get();

        //내 아이템 목록 업데이트하기
        ItemUserList itemUserList = new ItemUserList(user,item);

        itemUserListRepository.save(itemUserList);

        //아이템 리턴
        return getMyDress(user);
    }

    //내 아이템을 본다.
    @Override
    public BuyItemReturnDto getMyItemList(User user) {
        return getMyDress(user);
    }

    @Override
    public List<AchievementsDto> getMyAchievements(User user) {
        return getAchievements(user);
    }

    @Override
    public List<AchievementsDto> insertMyAchievements(User user,Long AchievementsIndex) {

        SuccessUserAchievements succeessUserAchievements = new SuccessUserAchievements();
        Achievements achievements = achievementsRepository.findById(AchievementsIndex).get();
        user = userRepository.findById(user.getUserIndex()).get();
        succeessUserAchievements.setAchievementsIndex(achievements);
        succeessUserAchievements.setUserIndex(user);
        successUserAchievementsRepository.save(succeessUserAchievements);

        return getAchievements(user);
    }

    //재사용을 위한 아이템 보기
    private BuyItemReturnDto getMyDress(User user){

        //아이템 넣어주고 return
        BuyItemReturnDto buyItemReturnDto = BuyItemReturnDto.builder().
                userMoney(user.getUserMoney()).
                userItems(new ArrayList()).
                build();

        List<ItemDto> list = buyItemReturnDto.getUserItems();

        for(ItemUserList li : itemUserListRepository.findItemUserListsByUserIndex(user)){
            Item item = itemRepository.findById(li.getItemIndex().getItemIndex()).get();
            ItemDto itemDto = ItemDto.builder().
                    itemIndex(item.getItemIndex()).
                    itemPrice(item.getItemPrice()).
                    itemCode(item.getItemCode()).
                    achievementsIndex(item.getAchievementsIndex()).
                    build();
            list.add(itemDto);
        }

        return buyItemReturnDto;

    }

    //재사용을 위한 업적 보기

    private List<AchievementsDto> getAchievements(User user){

        List<AchievementsDto> list = new ArrayList<>();

        for(SuccessUserAchievements su : successUserAchievementsRepository.findAllByUserIndex_UserIndex(user.getUserIndex())){
            Achievements achievements = achievementsRepository.findByAchievementsIndex(su.getAchievementsIndex().getAchievementsIndex());
            AchievementsDto achievementsDto = AchievementsDto.builder().
                    Achievements_content(achievements.getAchievementsContent()).
                    AchievementsIndex(achievements.getAchievementsIndex())
                    .build();
            list.add(achievementsDto);
        }
        return list;
    }


}
