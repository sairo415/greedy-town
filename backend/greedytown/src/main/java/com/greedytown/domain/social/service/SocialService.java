package com.greedytown.domain.social.service;

import com.greedytown.domain.social.dto.MyFriendDto;
import com.greedytown.domain.social.dto.RankingDto;
import com.greedytown.domain.user.model.User;

import java.util.List;

public interface SocialService {

    List<RankingDto> getUserRanking();

    Void insertFriend(User user, Long friendIndex);

    Boolean isFriend(User user,Long friendIndex);

    List<MyFriendDto> getMyFriendList(User user);
}
