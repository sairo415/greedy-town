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
    private Long userIndex;
    @Column(columnDefinition = "VARCHAR(255)", unique = true)
    private String userEmail;
    @Column(columnDefinition = "VARCHAR(255)")
    private String userPassword;
    @Column(columnDefinition = "VARCHAR(255)", unique = true)
    private String userNickName;
    private Long userMoney;
    @Column(columnDefinition = "TIME")
    private Date userClearTime;

}
