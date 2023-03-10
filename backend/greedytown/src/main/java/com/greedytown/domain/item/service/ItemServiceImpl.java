package com.greedytown.domain.item.service;

import com.greedytown.domain.item.dto.*;
import com.greedytown.domain.item.model.*;
import com.greedytown.domain.item.repository.*;
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
    private final WearingRepository wearingRepository;

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
        WearingDto wearingDto = new WearingDto();
        //아이템 리턴
        return getMyDress(user,wearingDto);
    }

    //내 아이템을 본다.
    @Override
    public BuyItemReturnDto getMyItemList(User user) {
        WearingDto wearingDto = new WearingDto();
        return getMyDress(user,wearingDto);
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


    @Override
    public BuyItemReturnDto changeMyDress(User user, WearingDto wearingDto) {

        return getMyDress(user,wearingDto);
    }




    //재사용을 위한 내 옷 보기
    private Void getMyWearingDress(User user, BuyItemReturnDto buyItemReturnDto,WearingDto wearingDto){


        Wearing wearing =  wearingRepository.findByUserIndex_UserIndex(user.getUserIndex());
        Item head = itemRepository.findByItemIndex(wearingDto.getWearingHead());
        Item hair = itemRepository.findByItemIndex(wearingDto.getWearingHair());
        Item dress =  itemRepository.findByItemIndex(wearingDto.getWearingDress());
        wearing.setWearingHead(head);
        wearing.setWearingHair(hair);
        wearing.setWearingDress(dress);

        wearingDto = WearingDto.builder().
                wearingIndex(wearing.getWearingIndex()).
                wearingHead(head==null ? null : wearing.getWearingHead().getItemIndex()).
                wearingHair(hair==null ? null :wearing.getWearingHair().getItemIndex()).
                wearingDress(dress==null ? null :wearing.getWearingDress().getItemIndex()).
                wearingHeadName(head==null ? "없음" :  itemRepository.findById(wearing.getWearingHead().getItemIndex()).get().getItemName()).
                wearingHairName(hair==null ? "없음" : itemRepository.findById(wearing.getWearingHair().getItemIndex()).get().getItemName()).
                wearingDressName(dress==null ? "없음" : itemRepository.findById(wearing.getWearingDress().getItemIndex()).get().getItemName()).
                build();

        buyItemReturnDto.setWearingDto(wearingDto);

        return null;
    }

    //재사용을 위한 아이템 보기
    private BuyItemReturnDto getMyDress(User user,WearingDto wearingDto){

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
                    itemName(item.getItemName()).
                    itemCode(item.getItemCode()).
                    achievementsIndex(item.getAchievementsIndex()).
                    build();
            //입고 있는 옷이면 가지고 있는 옷에서 빼주기.
            if(item.getItemIndex()==wearingDto.getWearingHead()) continue;
            else if(item.getItemIndex()==wearingDto.getWearingHair()) continue;
            else if(item.getItemIndex()==wearingDto.getWearingDress()) continue;
            list.add(itemDto);
        }


        getMyWearingDress(user,buyItemReturnDto,wearingDto);

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
