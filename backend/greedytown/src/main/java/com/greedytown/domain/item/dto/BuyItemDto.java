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


    private Long itemSeq;
    private Integer itemPrice;

    @Builder
    public BuyItemDto(Long itemSeq ,Integer itemPrice) {
        this.itemSeq = itemSeq;
        this.itemPrice = itemPrice;

    }

}
