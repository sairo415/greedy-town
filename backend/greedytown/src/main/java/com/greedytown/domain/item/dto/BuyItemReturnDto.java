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


    private List<Item> userItems;
    private Long userMoney;

    @Builder
    public BuyItemReturnDto(List userItems , Long userMoney ) {
        this.userItems = new ArrayList<>();
        this.userMoney = userMoney;
    }

}
