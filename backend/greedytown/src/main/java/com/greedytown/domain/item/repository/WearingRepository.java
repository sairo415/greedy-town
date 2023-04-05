package com.greedytown.domain.item.repository;

import com.greedytown.domain.item.model.Achievements;
import com.greedytown.domain.item.model.Wearing;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface WearingRepository extends JpaRepository<Wearing, Long> {

    List<Wearing> findAllByUserSeq_UserSeq(Long userSeq);

    long deleteAllByUserSeq_UserSeq(Long userSeq);
}
