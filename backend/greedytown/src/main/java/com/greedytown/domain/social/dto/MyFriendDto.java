package com.greedytown.domain.social.dto;

import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class MyFriendDto {

    private Long userSeq;
    private String userNickname;

    @Builder
    public MyFriendDto(Long userSeq , String userNickname ) {

        this.userSeq = userSeq;
        this.userNickname = userNickname;

    }

}
