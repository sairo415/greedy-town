package com.greedytown.domain.item.repository;

import com.greedytown.domain.item.model.ItemUserList;
import com.greedytown.domain.user.model.User;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface ItemUserListRepository extends JpaRepository<ItemUserList, Long> {
    List<ItemUserList> findItemUserListsByUserSeq_UserSeq(Long userSeq);

}
