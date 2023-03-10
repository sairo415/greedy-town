package com.greedytown.domain.user.service;

import com.greedytown.domain.user.dto.UserDto;

import java.util.Map;

public interface UserService {

    String insertUser(UserDto userDto);
    boolean duplicatedEmail(String userEmail);
    boolean duplicatedNickname(String userNickname);
    Map<String, String> reissue(String refreshToken);
}
