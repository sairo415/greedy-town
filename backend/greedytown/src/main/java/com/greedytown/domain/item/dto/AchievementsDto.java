package com.greedytown.domain.item.dto;

import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class AchievementsDto {

    private Long AchievementsSeq;
    private String AchievementsContent;

    @Builder
    public AchievementsDto(Long AchievementsSeq , String AchievementsContent ) {
        this.AchievementsSeq = AchievementsSeq;
        this.AchievementsContent = AchievementsContent;

    }

}
