package com.greedytown.domain.achievements.repository;

import com.greedytown.domain.achievements.model.Achievements;
import com.greedytown.domain.item.model.Item;
import org.springframework.data.jpa.repository.JpaRepository;

public interface AchievementsRepository extends JpaRepository<Achievements, Long> {

}
