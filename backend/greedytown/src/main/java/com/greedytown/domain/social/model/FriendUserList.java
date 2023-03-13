package com.greedytown.domain.social.model;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.greedytown.domain.item.model.ItemUserListPK;
import com.greedytown.domain.user.model.User;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.*;
import java.time.LocalDate;
import java.time.LocalDateTime;

@Entity
@Setter
@Getter
@AllArgsConstructor
@NoArgsConstructor
public class FriendUserList {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long frindSeq;

    @ManyToOne
    @JoinColumn(name="friend_from")
    private User friendFrom;


    @ManyToOne
    @JoinColumn(name="friend_to")
    private User friendTo;

    @JsonFormat(shape = JsonFormat.Shape.STRING, pattern = "yyyy-MM-dd", timezone = "Asia/Seoul")
    private LocalDate friendRequestDate;

    private Boolean friendAccept;






}
