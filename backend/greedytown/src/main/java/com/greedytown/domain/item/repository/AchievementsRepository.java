package com.greedytown.domain.item.repository;

import com.greedytown.domain.item.model.Achievements;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface AchievementsRepository extends JpaRepository<Achievements, Long> {
    
    Achievements findByAchievementsSeq(Long achievementsSeq);
}
