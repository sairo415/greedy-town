package com.greedytown.domain.item.dto;

import com.greedytown.domain.item.model.Achievements;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.JoinColumn;
import javax.persistence.OneToOne;


@Getter
@Setter
@NoArgsConstructor
public class ItemDto {

    private Long itemSeq;
    private String itemName;
    private Integer itemPrice;

    private Achievements achievementsSeq;

    private Integer itemColorSeq;
    private String itemColorName;

    private Integer itemTypeSeq;
    private String itemTypeName;



    @Builder
    public ItemDto(Long itemSeq, String itemName  , Integer itemPrice , Achievements achievementsSeq,Integer itemColorSeq,String itemColorName, Integer itemTypeSeq, String itemTypeName) {
        this.itemSeq = itemSeq;
        this.itemName = itemName;
        this.itemPrice = itemPrice;
        this.achievementsSeq = achievementsSeq;
        this.itemColorSeq =itemColorSeq;
        this.itemColorName = itemColorName;
        this.itemTypeSeq = itemTypeSeq;
        this.itemTypeName = itemTypeName;

    }

}
