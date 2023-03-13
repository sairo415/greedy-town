package com.greedytown.domain.social.model;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.greedytown.domain.user.model.User;
import lombok.*;

import javax.persistence.*;
import java.time.LocalDateTime;
import java.util.Date;


@Entity
@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor
public class Message {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long messageSeq;

    @JoinColumn(name = "message_from")
    @ManyToOne
    private User messageFrom;

    @JoinColumn(name = "message_to")
    @ManyToOne
    private User messageTo;
    private String messageContent;

    @JsonFormat(shape = JsonFormat.Shape.STRING, pattern = "yyyy-MM-dd'T'HH:mm:ss", timezone = "Asia/Seoul")
    private LocalDateTime messageWriteTime;
    private Boolean messageCheck;
}
