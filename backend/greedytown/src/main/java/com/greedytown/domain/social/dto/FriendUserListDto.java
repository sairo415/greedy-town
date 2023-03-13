package com.greedytown.domain.social.dto;

import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class FriendUserListDto {


    private Long friendSeq;

    private Long friendFrom;


    @Builder
    public FriendUserListDto(Long friendSeq, Long friendFrom ) {


        this.friendSeq = friendSeq;
        this.friendFrom = friendFrom;

    }
}
