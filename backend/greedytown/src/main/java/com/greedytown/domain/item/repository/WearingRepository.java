package com.greedytown.domain.item.repository;

import com.greedytown.domain.item.model.Achievements;
import com.greedytown.domain.item.model.Wearing;
import org.springframework.data.jpa.repository.JpaRepository;

public interface WearingRepository extends JpaRepository<Wearing, Long> {
    
    Wearing findByUserIndex_UserIndex(Long UserIndex);
}
