package com.greedytown.domain.item.dto;

import com.greedytown.domain.item.model.Item;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.ArrayList;
import java.util.List;


@Getter
@Setter
@NoArgsConstructor
public class BuyItemReturnDto {


    private List<ItemDto> userItems;
    private List<WearingDto> wearingDtos;
    private Long userMoney;

    @Builder
    public BuyItemReturnDto(Long userMoney, List userItems,List wearingDtos) {
        this.userMoney = userMoney;
        this.userItems = userItems;
        this.wearingDtos = wearingDtos;

        ;
    }

}
