package com.greedytown.domain.social.service;

import com.greedytown.domain.social.dto.MessageDto;
import com.greedytown.domain.social.dto.MyFriendDto;
import com.greedytown.domain.social.dto.MyMessageDto;
import com.greedytown.domain.social.dto.RankingDto;
import com.greedytown.domain.user.model.User;

import java.util.List;

public interface SocialService {

    List<RankingDto> getUserRanking();

    Void insertFriend(User user, Long friendIndex);

    Boolean isFriend(User user,Long friendIndex);

    List<MyFriendDto> getMyFriendList(User user);

    List<MyFriendDto> deleteMyFriend(User user,Long frinedIndex);

    Void sendMessage(User user, MessageDto messageDto);

    List<MyMessageDto> getMyMessage(User user);

    Void deleteMessage(Long messageIndex);

    Void deleteAllMessage(User user);

    Long getMyNewMessage(User user);
}
