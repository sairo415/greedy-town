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
    private String wearingHead;
    private String wearingTop;
    private String wearingBottom;


    @Builder
    public WearingDto(Long wearingIndex, String wearingHead , String wearingTop , String wearingBottom) {
        this.wearingIndex = wearingIndex;
        this.wearingHead = wearingHead;
        this.wearingTop = wearingTop;
        this.wearingBottom = wearingBottom;

    }

}
