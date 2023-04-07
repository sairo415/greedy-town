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
    private ItemDto itemDto;


    @Builder
        public WearingDto(ItemDto itemDto ) {
        this.itemDto = itemDto;

    }

}
