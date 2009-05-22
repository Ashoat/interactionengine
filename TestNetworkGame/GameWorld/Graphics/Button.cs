using InteractionEngine.Networking;
namespace TestNetworkGame.Graphics {

    public interface ButtonGraphics : InteractionEngine.UserInterface.Graphics {

        void returnAfterClick(Client client, object parameter);

        void setPosition(int x, int y);

        void onClick();

    }

}