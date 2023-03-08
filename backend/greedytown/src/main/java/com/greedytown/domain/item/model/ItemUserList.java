package com.greedytown.domain.item.model;

import com.greedytown.domain.achievements.model.Achievements;
import com.greedytown.domain.user.model.User;
import lombok.Getter;
import lombok.Setter;

import javax.persistence.*;

@Entity
@Setter
@Getter
public class ItemUserList {



    @Id
    @OneToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="user_index")
    private User userIndex;




}
