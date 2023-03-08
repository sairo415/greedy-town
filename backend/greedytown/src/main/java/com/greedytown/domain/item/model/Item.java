package com.greedytown.domain.item.model;

import com.greedytown.domain.achievements.model.Achievements;
import lombok.Getter;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Setter
@Getter
public class Item {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long itemIndex;

    private String itemName;

    private String itemCode;


    private Long itemPrice;

    @OneToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="block_user_index")
    private Achievements achievementsIndex;


}
