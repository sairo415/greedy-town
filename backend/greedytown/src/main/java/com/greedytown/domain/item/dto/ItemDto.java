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

    private Long itemIndex;
    private String itemName;
    private String itemCode;
    private Long itemPrice;

    @OneToOne
    @JoinColumn(name="achievements_index")
    private Achievements achievementsIndex;


    @Builder
    public ItemDto(Long itemIndex, String itemName , String itemCode , Long itemPrice , Achievements achievementsIndex) {
        this.itemIndex = itemIndex;
        this.itemName = itemName;
        this.itemCode = itemCode;
        this.itemPrice = itemPrice;
        this.achievementsIndex = achievementsIndex;
    }

}
