package com.greedytown.domain.item.repository;

import com.greedytown.domain.item.model.SuccessUserAchievements;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface SuccessUserAchievementsRepository extends JpaRepository<SuccessUserAchievements, Long> {
    List<SuccessUserAchievements> findAllByUserIndex_UserIndex(Long userIndex);


}
