package com.greedytown.domain.social.dto;

import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class MessageDto {


    private Long message_to;

    private String message_content;

    @Builder
    public MessageDto(Long message_to, String message_content ) {


        this.message_to = message_to;
        this.message_content = message_content;

    }
}
