package com.greedytown.domain.item.service;

import com.greedytown.domain.item.dto.BuyItemDto;
import com.greedytown.domain.item.dto.BuyItemReturnDto;
import com.greedytown.domain.item.dto.ItemDto;
import com.greedytown.domain.item.model.Item;
import com.greedytown.domain.item.repository.ItemRepository;
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
    public BuyItemReturnDto buyStoreItem(BuyItemDto BuyItemDto){

        //내 돈 없애기
        Long price = BuyItemDto.getItemPrice();

        //내 아이템 목록 업데이트하기
//        userRepository

        return null;
    }
}
