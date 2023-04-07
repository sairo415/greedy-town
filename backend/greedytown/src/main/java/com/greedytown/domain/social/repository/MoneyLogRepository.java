package com.greedytown.domain.social.repository;

import com.greedytown.domain.item.model.MoneyLog;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface MoneyLogRepository extends JpaRepository<MoneyLog, Long> {

}
