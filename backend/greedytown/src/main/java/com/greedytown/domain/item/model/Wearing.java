package com.greedytown.domain.item.model;

import com.greedytown.domain.user.model.User;
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
    @OneToOne
    private User userIndex;

    @JoinColumn(name="wearing_head")
    @ManyToOne
    private Item wearingHead;
    @JoinColumn(name="wearing_hair")
    @ManyToOne
    private Item wearingHair;
    @JoinColumn(name="wearing_dress")
    @ManyToOne
    private Item wearingDress;




}
