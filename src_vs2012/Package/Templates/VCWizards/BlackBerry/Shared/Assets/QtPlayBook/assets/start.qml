import QtQuick 1.0

Rectangle {
    id: screen

    width: 1024;
    height: 600;

    Text {
        anchors {
            horizontalCenter: parent.horizontalCenter
            verticalCenter: parent.verticalCenter
        }
        text: "Hello World!"
    }
}
