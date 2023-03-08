package com.greedytown.domain.item.dto;

import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;


@Getter
@Setter
@NoArgsConstructor
public class ItemDto {

    private Long itemIndex;
    private String itemName;
    private String itemCode;

    private Long itemPrice;


    @Builder
    public ItemDto(Long itemIndex, String itemName , String itemCode , Long itemPrice ) {
        this.itemIndex = itemIndex;
        this.itemName = itemName;
        this.itemCode = itemCode;
        this.itemPrice = itemPrice;
    }

}
