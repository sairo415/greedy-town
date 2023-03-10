package com.greedytown.domain.user.service;

import com.greedytown.domain.item.model.Wearing;
import com.greedytown.domain.item.repository.WearingRepository;
import com.greedytown.domain.user.dto.UserDto;
import com.greedytown.domain.user.model.User;
import com.greedytown.domain.user.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class UserServiceImpl implements UserService {

    private final UserRepository userRepository;
    private final BCryptPasswordEncoder bCryptPasswordEncoder;

    private final WearingRepository wearingRepository;

    @Override
    public String insertUser(UserDto userDto) {
        User wearingUser = null;
        String message = "";
        userDto.setUserPassword(bCryptPasswordEncoder.encode(userDto.getUserPassword()));
        User user = User.builder()
                .userNickname(userDto.getUserNickname())
                .userEmail(userDto.getUserEmail())
                .userPassword(userDto.getUserPassword())
                .build();
        try {
            wearingUser = userRepository.save(user);
        } catch (Exception e) {
            message = "회원가입 실패";
            return message;
        }
        try {
            Wearing wearing = new Wearing();
            wearing.setUserIndex(wearingUser);
            wearingRepository.save(wearing);

        } catch (Exception e){
            message = "옷입기 실패";
            return message;
        }
        message = "다 성공";

        return message;
    }



    @Override
    public boolean duplicatedEmail(String userEmail) {
        User user = userRepository.findByUserEmail(userEmail);
        if(user == null) return false;
        return true;
    }

    @Override
    public boolean duplicatedNickname(String userNickname) {
        User user = userRepository.findByUserNickname(userNickname);
        if(user == null) return false;
        return true;
    }
}
