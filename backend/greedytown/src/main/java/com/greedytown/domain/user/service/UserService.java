package com.greedytown.domain.user.service;

import com.greedytown.domain.user.dto.*;
import com.greedytown.domain.user.model.User;

import java.util.Map;

public interface UserService {

    String insertUser(UserDto userDto);
    boolean duplicatedEmail(String userEmail);
    boolean duplicatedNickname(String userNickname);
    Map<String, String> reissue(TokenDto tokenDto);
    Map<String, String> logout(User user, String aceessToken);

    StatDto updateStat(User user, StatDto statDto);

    UserInfoDto getUserInfo(User user);

    Map<String, String> earnMoney(User user, EarnMoneyDto earnMoneyDto);
}
