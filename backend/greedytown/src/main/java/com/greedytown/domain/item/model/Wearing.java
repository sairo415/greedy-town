package com.greedytown.domain.item.model;

import lombok.Getter;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Setter
@Getter
public class Wearing {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long wearingIndex;

    @JoinColumn(name="user_index")
    private Long userIndex;

    private String itemCode;


    private Long itemPrice;

    @OneToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="ahichievements_index")
    private Achievements achievementsIndex;


}
