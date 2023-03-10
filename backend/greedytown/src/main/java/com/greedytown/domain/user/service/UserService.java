package com.greedytown.domain.user.service;

import com.greedytown.domain.user.dto.UserDto;

public interface UserService {

    boolean insertUser(UserDto userDto);
    boolean duplicatedEmail(String userEmail);
    boolean duplicatedNickname(String userNickname);
}
