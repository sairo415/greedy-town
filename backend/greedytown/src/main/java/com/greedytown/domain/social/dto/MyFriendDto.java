package com.greedytown.domain.social.dto;

import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class MyFriendDto {

    private Long userIndex;
    private String userNickname;

    @Builder
    public MyFriendDto(Long userIndex , String userNickname ) {

        this.userIndex = userIndex;
        this.userNickname = userNickname;

    }

}
