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
public class WearingDto {

    private Long wearingIndex;
    private Long wearingHead;
    private Long wearingHair;
    private Long wearingDress;

    private String wearingHeadName;
    private String wearingHairName;
    private String wearingDressName;


    @Builder
        public WearingDto(Long wearingIndex, Long wearingHead , Long wearingHair , Long wearingDress,String wearingHeadName, String wearingHairName, String wearingDressName) {
        this.wearingIndex = wearingIndex;
        this.wearingHead = wearingHead;
        this.wearingHair = wearingHair;
        this.wearingDress = wearingDress;
        this.wearingHeadName  = wearingHeadName;
        this.wearingHairName = wearingHairName;
        this.wearingDressName = wearingDressName;
    }

}
