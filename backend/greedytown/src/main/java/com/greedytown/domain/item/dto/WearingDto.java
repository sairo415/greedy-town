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

    private Long wearingSeq;
    private ItemDto itemDto;


    @Builder
        public WearingDto(Long wearingSeq, ItemDto itemDto ) {
        this.wearingSeq = wearingSeq;
        this.itemDto = itemDto;

    }

}
