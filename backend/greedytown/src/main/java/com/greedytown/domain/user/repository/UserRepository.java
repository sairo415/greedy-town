package com.greedytown.domain.user.repository;

import com.greedytown.domain.user.model.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface UserRepository extends JpaRepository<User, Long> {

    List<User> findAllByOrderByUserClearTimeDesc();

    User findUserByUserIndex(Long userIndex);

}
