package com.greedytown.domain.item.service;


import com.greedytown.domain.item.dto.*;
import com.greedytown.domain.user.model.User;

import java.util.List;

public interface ItemService {

    List<ItemDto> getStoreItems();

    BuyItemReturnDto buyStoreItem(BuyItemDto buyItemDto);

    BuyItemReturnDto getMyItemList (User user);

    List<AchievementsDto> getMyAchievements(User user);

    List<AchievementsDto> insertMyAchievements(User user,Long AchievementsIndex);

    BuyItemReturnDto changeMyDress(User user, WearingDto wearingDto);
}
