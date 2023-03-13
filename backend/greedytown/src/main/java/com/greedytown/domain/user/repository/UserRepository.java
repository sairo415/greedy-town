package com.greedytown.domain.user.repository;

import com.greedytown.domain.user.model.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface UserRepository extends JpaRepository<User, Long> {

    User findUserByUserSeq(Long userSeq);

    User findByUserEmail(String userEmail);

    User findByUserNickname(String userNickname);
}
