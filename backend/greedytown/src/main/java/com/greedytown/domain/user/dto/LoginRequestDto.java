package com.greedytown.domain.user.dto;

import lombok.Data;

@Data
public class LoginRequestDto {

    private String userEmail;
    private String userPassword;
}
