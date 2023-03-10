package com.greedytown.domain.item.repository;

import com.greedytown.domain.item.model.Item;
import org.springframework.data.jpa.repository.JpaRepository;

public interface ItemRepository extends JpaRepository<Item, Long> {

    Item findByItemIndex(Long itemIndex);

}
