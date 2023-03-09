package com.greedytown.domain.social.dto;

import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.Date;


@Getter
@Setter
@NoArgsConstructor
public class RankingDto {

    private String userNickname;
    private String clearTime;


    @Builder
    public RankingDto(String userNickname , String clearTime ) {

        this.userNickname = userNickname;
        this.clearTime = clearTime;

    }

}
