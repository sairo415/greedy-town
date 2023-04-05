package com.greedytown.domain.item.service;

import com.greedytown.domain.item.dto.*;
import com.greedytown.domain.item.model.*;
import com.greedytown.domain.item.repository.*;
import com.greedytown.domain.social.repository.MoneyLogRepository;
import com.greedytown.domain.user.model.User;
import com.greedytown.domain.user.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.Date;
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
    private final MoneyLogRepository moneyLogRepository;

    //전체 아이템 보기
    @Override
    public List<ItemDto> getStoreItems() {
        List<ItemDto> itemDtoList = new ArrayList<>();
        for(Item item : itemRepository.findAll()){
            ItemDto itemDto = ItemDto.builder().
                    itemSeq(item.getItemSeq()).
                    itemPrice(item.getItemPrice()).
                    itemName(item.getItemName()).
                    itemImage(item.getItemImage()).build();
            itemDtoList.add(itemDto);
        }
        return itemDtoList;
    }
    //아이템 구입하기
    public BuyItemReturnDto buyStoreItem(BuyItemDto buyItemDto, User user){

        //Integer price = buyItemDto.getItemPrice();
        Item item = itemRepository.findById(buyItemDto.getItemSeq()).get();

        // 현금 흐름 반영
        // 발생 시각 new Date(), 아이템 정보, user seq, 금액 insert
        MoneyLog moneyLog = MoneyLog.builder()
                .moneyLogTime(new Date())
                .moneyLogIteminfo(item)
                .userSeq(user)
                .moneyLogMoney(-item.getItemPrice().longValue())
                .build();
        moneyLogRepository.save(moneyLog);

        //내 돈 없애기
        user.setUserMoney(user.getUserMoney()-item.getItemPrice().longValue());
        userRepository.save(user);
        //내 아이템 목록 업데이트하기
        ItemUserList itemUserList = new ItemUserList(user,item);

        itemUserListRepository.save(itemUserList);
        WearingDto wearingDto = new WearingDto();

        //아이템 리턴
        BuyItemReturnDto buyItemReturnDto = new BuyItemReturnDto();
        getMyDress(user,buyItemReturnDto);
        return getMyWearingDress(user,buyItemReturnDto);

    }

    //내 아이템을 본다.
    @Override
    public BuyItemReturnDto getMyItemList(User user) {
        //지금 입은 옷 찾기
        BuyItemReturnDto buyItemReturnDto = new BuyItemReturnDto();
        //지금 입은 옷 보기
        getMyWearingDress(user,buyItemReturnDto);
        //전체 아이템 보기
        return getMyDress(user,buyItemReturnDto);

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


    @Transactional
    @Override
    public BuyItemReturnDto changeMyDress(User user, List<WearingDto> wearingDtos) {

        BuyItemReturnDto buyItemReturnDto = new BuyItemReturnDto();
        //내 전체 아이템
        getMyDress(user,buyItemReturnDto);
        //여기서 내 옷 입어주기

        wearingRepository.deleteAllByUserSeq_UserSeq(user.getUserSeq());
        for(WearingDto wearingNow : wearingDtos) {
            //System.out.println(wearingNow.getItemDto().getItemTypeSeq().getClass()+ " 이치헌");
            //System.out.println(user.getUserSeq().getClass()+ " 이승진");
            Wearing wearing = new Wearing();
            Item item = itemRepository.findByItemSeq(wearingNow.getItemDto().getItemSeq());
            wearing.setItemSeq(item);
            wearing.setUserSeq(user);
            wearingRepository.save(wearing);
        }

        return getMyWearingDress(user,buyItemReturnDto);
    }




    //재사용을 위한 내 옷 보기
    private BuyItemReturnDto getMyWearingDress(User user,BuyItemReturnDto buyItemReturnDto){

        buyItemReturnDto.setWearingDtos(new ArrayList<>());

        List<WearingDto> wearingList = buyItemReturnDto.getWearingDtos();

        List<Wearing> wearings = wearingRepository.findAllByUserSeq_UserSeq(user.getUserSeq());
        System.out.println(wearings.get(0));
        //내가 입고 있는 아이템 정보 습득
        for(Wearing wearing : wearings){
            //아이템 정보
            System.out.println(wearing.getItemSeq());
            System.out.println(wearing.getItemSeq().getItemSeq());
            Item item = itemRepository.findByItemSeq(wearing.getItemSeq().getItemSeq());

            ItemDto itemDto = ItemDto.builder().
                              itemSeq(item.getItemSeq()).
                              build();

            WearingDto wearingDto = WearingDto.builder().
                                    itemDto(itemDto).
                                    build();

            wearingList.add(wearingDto);
        }

        return buyItemReturnDto;
    }

    //재사용을 위한 아이템 보기
    private BuyItemReturnDto getMyDress(User user, BuyItemReturnDto buyItemReturnDto){


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
                    itemImage(item.getItemImage()).
                    build();
            //입고 있는 옷이면 가지고 있는 옷에서 빼주기.
            list.add(itemDto);
        }
        return buyItemReturnDto;
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
