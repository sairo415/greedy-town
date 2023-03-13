package com.greedytown.domain.item.model;

import lombok.Getter;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Setter
@Getter
public class Item {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long itemSeq;

    private String itemName;


    private Integer itemPrice;

    @OneToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="ahichievementsSeq")
    private Achievements achievementsSeq;

    @ManyToOne
    @JoinColumn(name="itemTypeSeq")
    private ItemType itemTypeSeq;

    @ManyToOne
    @JoinColumn(name="itemColorSeq")
    private ItemColor itemColorSeq;


}
