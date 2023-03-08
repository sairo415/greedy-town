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
public class BuyItemDto {


    private Long itemIndex;
    private Long itemPrice;
    private Long userIndex;

    @Builder
    public BuyItemDto(Long itemIndex ,Long itemPrice, Long userIndex ) {
        this.itemIndex = itemIndex;
        this.itemPrice = itemPrice;
        this.userIndex = userIndex;

    }

}
