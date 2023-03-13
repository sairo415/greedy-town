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
                    itemSeq(item.getItemSeq()).
                    itemPrice(item.getItemPrice()).
                    itemName(item.getItemName()).build();
            itemDtoList.add(itemDto);
        }
        return itemDtoList;
    }
    //아이템 구입하기
    public BuyItemReturnDto buyStoreItem(BuyItemDto buyItemDto){

        //내 돈 없애기
        Integer price = buyItemDto.getItemPrice();
        User user = userRepository.findById(buyItemDto.getUserSeq()).get();
        user.setUserMoney(user.getUserMoney()-price);
        Item item = itemRepository.findById(buyItemDto.getItemSeq()).get();
        //내 아이템 목록 업데이트하기
        ItemUserList itemUserList = new ItemUserList(user,item);

        itemUserListRepository.save(itemUserList);
        WearingDto wearingDto = new WearingDto();
        //아이템 리턴
        return getMyWearingDress(user,wearingDto);
    }

    //내 아이템을 본다.
    @Override
    public BuyItemReturnDto getMyItemList(User user) {
        //지금 입은 옷 찾기
        WearingDto wearingDto = new WearingDto();

        return getMyWearingDress(user,wearingDto);
    }

    @Override
    public List<AchievementsDto> getMyAchievements(User user) {
        return getAchievements(user);
    }

    @Override
    public List<AchievementsDto> insertMyAchievements(User user,Long AchievementsIndex) {

        SuccessUserAchievements succeessUserAchievements = new SuccessUserAchievements();
        Achievements achievements = achievementsRepository.findById(AchievementsIndex).get();
        user = userRepository.findById(user.getUserSeq()).get();
        succeessUserAchievements.setAchievementsSeq(achievements);
        succeessUserAchievements.setUserSeq(user);
        successUserAchievementsRepository.save(succeessUserAchievements);

        return getAchievements(user);
    }


    @Override
    public BuyItemReturnDto changeMyDress(User user, WearingDto wearingDto) {

        return getMyWearingDress(user,wearingDto);
    }




    //재사용을 위한 내 옷 보기
    private BuyItemReturnDto getMyWearingDress(User user ,WearingDto wearingDto){

        BuyItemReturnDto buyItemReturnDto = new BuyItemReturnDto();

        Wearing wearing =  wearingRepository.findByUserSeq_UserSeq(user.getUserSeq());
        Item item = itemRepository.findByItemSeq(wearingDto.getItemDto().getItemSeq());

        wearing.setItemSeq(item);

        ItemDto itemDto = ItemDto.builder().
                          itemSeq(item.getItemSeq()).
                          itemName(item.getItemName()).
                          itemColorSeq(item.getItemColorSeq().getItemColorSeq()).
                          itemColorName(item.getItemColorSeq().getItemColorName()).
                          itemTypeSeq(item.getItemTypeSeq().getItemTypeSeq()).
                          itemTypeName(item.getItemTypeSeq().getItemTypeName()).
                          build();

        wearingDto = WearingDto.builder().
                wearingSeq(wearing.getWearingSeq()).
                itemDto(itemDto).
                build();

        buyItemReturnDto.setWearingDto(wearingDto);

        getMyDress(user,wearingDto,buyItemReturnDto);

        return buyItemReturnDto;
    }

    //재사용을 위한 아이템 보기
    private Void getMyDress(User user,WearingDto wearingDto,BuyItemReturnDto buyItemReturnDto){

        buyItemReturnDto.setUserMoney(user.getUserMoney());
        buyItemReturnDto.setUserItems(new ArrayList());
        //아이템 넣어주고 return

        List<ItemDto> list = buyItemReturnDto.getUserItems();

        for(ItemUserList li : itemUserListRepository.findItemUserListsByUserSeq_UserSeq(user.getUserSeq())){
            Item item = itemRepository.findById(li.getItemSeq().getItemSeq()).get();
            ItemDto itemDto = ItemDto.builder().
                    itemSeq(item.getItemSeq()).
                    itemPrice(item.getItemPrice()).
                    itemName(item.getItemName()).
                    achievementsSeq(item.getAchievementsSeq()).
                    build();
            //입고 있는 옷이면 가지고 있는 옷에서 빼주기.
            if(item.getItemSeq()==wearingDto.getItemDto().getItemSeq()) continue;

            list.add(itemDto);
        }


        return null;

    }

    //재사용을 위한 업적 보기

    private List<AchievementsDto> getAchievements(User user){

        List<AchievementsDto> list = new ArrayList<>();

        for(SuccessUserAchievements su : successUserAchievementsRepository.findAllByUserSeq_UserSeq(user.getUserSeq())){
            Achievements achievements = achievementsRepository.findByAchievementsSeq(su.getAchievementsSeq().getAchievementsSeq());
            AchievementsDto achievementsDto = AchievementsDto.builder().
                    AchievementsContent(achievements.getAchievementsContent()).
                    AchievementsSeq(achievements.getAchievementsSeq())
                    .build();
            list.add(achievementsDto);
        }
        return list;
    }


}
