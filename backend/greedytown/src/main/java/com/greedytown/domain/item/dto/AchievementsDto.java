package com.greedytown.domain.item.dto;

import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class AchievementsDto {

    private Long AchievementsIndex;
    private String Achievements_content;

    @Builder
    public AchievementsDto(Long AchievementsIndex , String Achievements_content ) {
        this.AchievementsIndex = AchievementsIndex;
        this.Achievements_content = Achievements_content;

    }

}
