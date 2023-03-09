package com.greedytown.domain.social.repository;

import com.greedytown.domain.social.model.FriendUserList;
import com.greedytown.domain.user.model.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface FriendUserListRepository extends JpaRepository<FriendUserList, Long> {

    Boolean existsByUserIndexA_UserIndexAndUserIndexB_UserIndex(Long userIndexA, Long userIndexB);

    Boolean existsByUserIndexB_UserIndexAndUserIndexA_UserIndex(Long userIndexB, Long userIndexA);

    List<FriendUserList> findAllByUserIndexA_userIndex(Long userIndexA);
    List<FriendUserList> findAllByUserIndexB_userIndex(Long userIndexB);
}
