package com.greedytown.domain.social.dto;

import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class MyMessageDto {

    private Long messageIndex;
    private Long messageFrom;
    private String messageFromNickname;
    private String messageContent;

    @Builder
    public MyMessageDto(Long messageIndex,Long messageFrom,String messageFromNickname, String messageContent) {

        this.messageIndex = messageIndex;
        this.messageFrom = messageFrom;
        this.messageFromNickname = messageFromNickname;
        this.messageContent = messageContent;

    }
}
