package com.greedytown.domain.item.service;


import com.greedytown.domain.item.dto.BuyItemDto;
import com.greedytown.domain.item.dto.BuyItemReturnDto;
import com.greedytown.domain.item.dto.ItemDto;

import java.util.List;

public interface ItemService {

    List<ItemDto> getStoreItems();

    BuyItemReturnDto buyStoreItem(BuyItemDto buyItemDto);

}
