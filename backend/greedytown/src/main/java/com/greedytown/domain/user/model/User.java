package com.greedytown.domain.user.model;

import lombok.*;

import javax.persistence.*;
import java.util.Date;

@Entity
@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor
@Builder
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long userSeq;
    @Column(columnDefinition = "VARCHAR(100)")
    private String userPassword;
    @Column(columnDefinition = "VARCHAR(100)", unique = true)
    private String userEmail;

    @Column(columnDefinition = "VARCHAR(30)", unique = true)
    private String userNickname;
    @Column(columnDefinition = "BIGINT DEFAULT 0")
    private Long userMoney;
    @Column(columnDefinition = "DATE")
    private Date userJoinDate;

}
