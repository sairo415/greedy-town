package com.greedytown.domain.user.dto;

import lombok.Builder;
import lombok.Data;

import java.util.Date;

@Data
@Builder
public class UserInfoDto {

    private String userNickname;
    private Long userMoney;
    private Date userJoinDate;
}
