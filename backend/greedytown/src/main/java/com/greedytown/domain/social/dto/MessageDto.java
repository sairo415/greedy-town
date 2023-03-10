package com.greedytown.domain.social.dto;

import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class MessageDto {


    private Long messageTo;
    private String messageContent;

    @Builder
    public MessageDto(Long messageTo, String messageContent ) {


        this.messageTo = messageTo;
        this.messageContent = messageContent;

    }
}
