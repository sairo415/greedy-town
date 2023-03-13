package com.greedytown.domain.social.repository;

import com.greedytown.domain.social.model.FriendUserList;
import com.greedytown.domain.social.model.Stat;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface StatRepository extends JpaRepository<Stat, Long> {

    List<Stat> findAllByOrderByUserClearTimeDesc();
}
